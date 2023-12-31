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
	import loadingStore from "$lib/stores/loadingStore.store";

	let currentChat: Chat;
	let elmSlot: HTMLDivElement;
	let searchString = "";
	let chats: Chat[] = [];
	let searchChats: Chat[] = [];
	export let data: PageData;
	let toggleSideBar = true;

	const searchDebounce = useDebounce(500, async () => {
		await populateChats();
		searchChats = chats.filter((chat) => {
			return chat.user.email.includes(searchString);
		});

		if (searchString.length < 5) return;

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

	async function populateChats() {
		loadingStore.set(true);
		const data: Chat[] = await fetchChats();
		chats = data.sort((a, b) => {
			return (
				new Date(b.lastMessage.createdAt!).getTime() -
				new Date(a.lastMessage.createdAt!).getTime()
			);
		});
		searchChats = chats;
		loadingStore.set(false);
	}

	function reciveMessageLayout(message: Message) {
		if (message.senderId === get(authStore)!.id) {
			chats = [
				{
					user: message.receiver!,
					lastMessage: message,
				},
				...chats.filter((chat) => chat.user.id !== message.receiverId),
			];
		} else {
			chats = [
				{
					user: message.sender!,
					lastMessage: message,
				},
				...chats.filter((chat) => chat.user.id !== message.senderId),
			];
		}
		searchChats = chats;
		const lastChat = currentChat;

		if (lastChat.user.id === message.senderId) {
			currentChat = chats[0];
		}
	}

	onDestroy(() => {
		if (connection) {
			connection.off("ReceiveMessage", reciveMessageLayout);
			connection.stop();
		}
	});

	$: if ($page.params.email) {
		const chat = searchChats.find(
			(chat) => chat.user.id === $page.params.email
		);
		if (chat) {
			currentChat = chat;
		}
	}

	$: connection = data.connection;
	$: if (connection) {
		// clear any previous event handlers
		connection.off("ReceiveMessage", reciveMessageLayout);

		// on receive message
		connection.on("ReceiveMessage", reciveMessageLayout);
	}

	populateChats();
</script>

<div
	class="fixed flex flex-row justify-between left-0 right-0 top-0 md:hidden bg-surface-50-900-token ring-2 p-2 px-4 z-10"
>
	<button on:click={() => (toggleSideBar = !toggleSideBar)}>
		<svg
			xmlns="http://www.w3.org/2000/svg"
			fill="none"
			viewBox="0 0 24 24"
			stroke-width="1.5"
			stroke="currentColor"
			class="w-6 h-6"
		>
			<path
				stroke-linecap="round"
				stroke-linejoin="round"
				d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5"
			/>
		</svg>
	</button>
	<p class="flex-1 text-center">
		{currentChat?.user?.email ?? "Select a chat"}
	</p>
</div>

<div class="chat w-full flex flex-row">
	<!-- Navigation -->
	<div
		class="max-w-xs border-r border-surface-500/30 bg-surface-50-900-token fixed md:relative min-h-screen z-10 transition-all md:visible md:opacity-100"
		class:invisible={toggleSideBar}
		class:opacity-0={toggleSideBar}
	>
		<!-- Header -->
		<header class="border-b border-surface-500/30 p-4 flex flex-col gap-2">
			<div class="flex flex-row items-center justify-between gap-2">
				<span class="flex-1 text-secondary-800-100-token">
					{$authStore?.email}
				</span>
				<button
					class="btn variant-soft-error"
					on:click={() => {
						authStore.set(null);
						goto("/");
					}}
				>
					Logout
				</button>
			</div>
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
						on:click={() => {
							// 64 encode email
							goto(`/chats/${chat.user.id}`);
							toggleSideBar = true;
						}}
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
	<!-- svelte-ignore a11y-no-static-element-interactions -->
	<!-- svelte-ignore a11y-click-events-have-key-events -->
	<div
		class="contents"
		on:click={() => (toggleSideBar = true)}
		bind:this={elmSlot}
	>
		<slot />
	</div>
</div>
