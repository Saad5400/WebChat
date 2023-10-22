<script lang="ts">
	import { goto } from "$app/navigation";
	import fetchChats from "$lib/api/chats/get";
	import searchUsers from "$lib/api/users/search";
	import useDebounce from "$lib/hooks/useDebounce";
	import { ListBox, ListBoxItem } from "@skeletonlabs/skeleton";
	import { page } from "$app/stores";
	import { onMount } from "svelte";

	// Navigation List
	let currentChat: Chat;

	$: {
		if (currentChat) {
			goto(`/chats/${currentChat.user.email}`);
		}
	}

	let searchString = "";
	let chats: Chat[] = [];
	$: searchChats = chats;

	async function populateChats() {
		const data: Chat[] = await fetchChats();
		chats = data.sort((a, b) => {
			return (
				new Date(b.lastMessage.createdAt!).getTime() -
				new Date(a.lastMessage.createdAt!).getTime()
			);
		});
	}

	populateChats();

	const searchDebounce = useDebounce(500, async () => {
		await populateChats();
		searchChats = chats.filter((chat) => {
			return chat.user.email.includes(searchString);
		});

		if (searchString.length < 3) return;

		const res = searchUsers(searchString);
		res.then((users: User[]) => {
			for (const user of users) {
				if (searchChats.find((chat) => chat.user.email === user.email))
					continue;
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
	});
</script>

<div class="chat w-full min-h-screen grid grid-cols-1 lg:grid-cols-[30%_1fr]">
	<!-- Navigation -->
	<div
		class="hidden lg:grid grid-rows-[auto_1fr_auto] border-r border-surface-500/30"
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
					<ListBoxItem bind:group={currentChat} name="people" value={chat}>
						<!-- <svelte:fragment slot="lead">
							<Avatar
								src="https://i.pravatar.cc/?img={person.avatar}"
								width="w-8"
							/>
						</svelte:fragment> -->
						{chat.user.email}
						<small class="opacity-50 text-xs m-0 ms-2 p-0">
							{chat.lastMessage.text.slice(0, 20) +
								(chat.lastMessage.text.length > 20 ? " .." : "")}
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
