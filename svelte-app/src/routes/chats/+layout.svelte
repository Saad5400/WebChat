<script lang="ts">
    import { goto } from "$app/navigation";
	import fetchChats from "$lib/api/chats/chats";
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
	let searchChats: Chat[] = [];

	fetchChats().then((chats) => {
		console.log(chats);
	});

	const searchDebounce = useDebounce(500, () => {
		searchChats = chats.filter((chat) => {
			return chat.user.email.includes(searchString);
		});
		if (searchString.length < 3) return;
		const res = searchUsers(searchString);
		res.then((users: User[]) => {
			for (const user of users) {
				searchChats = [
					...searchChats,
					{
						user: user,
						lastMessage: {
							id: "",
							text: "",
							timestamp: "",
						},
					},
				];
			}
			console.log(searchChats);
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
					</ListBoxItem>
				{/each}
			</ListBox>
		</div>
		<!-- Footer -->
		<!-- <footer class="border-t border-surface-500/30 p-4">(footer)</footer> -->
	</div>
		<slot />
</div>
