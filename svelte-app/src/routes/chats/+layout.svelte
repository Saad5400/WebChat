<script lang="ts">
	import { goto } from "$app/navigation";
	import fetchChats from "$lib/api/chats/get";
	import searchUsers from "$lib/api/users/search";
	import useDebounce from "$lib/hooks/useDebounce";
	import { ListBox, ListBoxItem } from "@skeletonlabs/skeleton";
	import authStore from "$lib/stores/authStore.store.";
	import { get } from "svelte/store";
	import type { PageData } from "./$types";
	import { page } from "$app/stores";
	import { onDestroy } from "svelte";

	// Navigation List
	let currentChat: Chat;

	$: if (currentChat) {
		goto(`/chats/${currentChat.user.email}`);
	} else if ($page.params.email) {
		const chat = searchChats.find(
			(chat) => chat.user.email === $page.params.email
		);
		if (chat) {
			currentChat = chat;
		} else {
			goto("/chats");
		}
	}

	let searchString = "";
	let chats: Chat[] = [];
	let searchChats: Chat[] = [];

	async function populateChats() {
		const data: Chat[] = await fetchChats();
		chats = data.sort((a, b) => {
			return (
				new Date(b.lastMessage.createdAt!).getTime() -
				new Date(a.lastMessage.createdAt!).getTime()
			);
		});
		searchChats = chats;
	}

	populateChats();

	const searchDebounce = useDebounce(500, async () => {
		await populateChats();
		searchChats = chats.filter((chat) => {
			return chat.user.email.includes(searchString);
		});

		if (searchString.length < 3) return;

		const users: User[] = await searchUsers(searchString);
		for (const user of users) {
			if (searchChats.find((chat) => chat.user.email === user.email))
				continue;
			if (user.id == get(authStore)!.id) continue;

			searchChats = [
				...searchChats,
				{
					user: user,
					lastMessage: {
						text: "",
					},
				},
			];
		}
	});

	export let data: PageData;

	function reciveMessageLayout(message: Message) {
		console.log("LAYOUT");

		const lastChat = currentChat;

		chats = [
			{
				user: message.sender!,
				lastMessage: message,
			},
			...chats.filter((chat) => chat.user.id !== message.senderId),
		];

		if (lastChat.user.id === message.senderId) {
			currentChat = chats[0];
		}

		searchChats = chats;
	}

	$: connection = data.connection;
	$: if (connection) {
		// clear any previous event handlers
		connection.off("ReceiveMessage", reciveMessageLayout);

		// on receive message
		connection.on("ReceiveMessage", reciveMessageLayout);
	}

	onDestroy(() => {
		if (connection) {
			connection.off("ReceiveMessage", reciveMessageLayout);
		}
	});
</script>

<div class="chat w-full min-h-screen grid grid-cols-1 md:grid-cols-[30%_1fr]">
	<!-- Navigation -->
	<div
		class="hidden md:grid grid-rows-[auto_1fr_auto] border-r border-surface-500/30"
	>
		<!-- Header -->
		<header class="border-b border-surface-500/30 p-4">
			<input
				class="input"
				type="search"
				placeholder="Search..."
				bind:value={searchString}
				on:input={searchDebounce}
			/>
		</header>
		<!-- List -->
		<div class="p-4 space-y-4 overflow-y-auto">
			<small class="opacity-50"> Contacts </small>
			<ListBox active="variant-filled-primary">
				{#each searchChats as chat}
					<ListBoxItem
						bind:group={currentChat}
						name="people"
						value={chat}
					>
						<!-- <svelte:fragment slot="lead">
							<Avatar
								src="https://i.pravatar.cc/?img={person.avatar}"
								width="w-8"
							/>
						</svelte:fragment> -->
						{chat.user.email}
						<small class="opacity-50 text-xs m-0 ms-2 p-0">
							{chat.lastMessage.text.slice(0, 20) +
								(chat.lastMessage.text.length > 20
									? " .."
									: "")}
						</small>
					</ListBoxItem>
				{/each}
			</ListBox>
		</div>
		<!-- Footer -->
		<!-- <footer class="border-t border-surface-500/30 p-4">(footer)</footer> -->
	</div>
	<slot />
</div>
