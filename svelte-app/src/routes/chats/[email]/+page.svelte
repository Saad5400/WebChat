<script lang="ts">
	import { onMount } from "svelte";
	import { page } from "$app/stores";
	import getUsers from "$lib/api/users/get";
	import { goto } from "$app/navigation";
	import postNewMessage from "$lib/api/messages/post";
	import getMessages from "$lib/api/messages/get";
	import type { PageData } from "./$types";
	import { get } from "svelte/store";
	import authStore from "$lib/stores/authStore.store.";

	let elemChat: HTMLElement;
	let elemChatContent: HTMLElement;

	let currentMessage = "";
	let currentUser: User;
	let messageFeed: Message[] = [];

	async function populateUser(email: string) {
		let users: User[] = await (await getUsers(email)).json();
		if (users.length === 0) {
			goto("/chats");
		}
		currentUser = users[0];
	}

	async function populateMessages() {
		const messages = await getMessages(currentUser.id);
		// convert created at to timestamp
		messageFeed = messages.map((message: Message) => {
			return {
				...message,
				createdAt: getTimestamp(message.createdAt),
			};
		});
	}

	page.subscribe(async (value) => {
		currentMessage = "";
		currentUser = {} as User;
		messageFeed = [];

		if (value.params.email) {
			await populateUser(value.params.email);
			await populateMessages();
			scrollChatBottom();
		} else {
			goto("/chats");
		}
	});

	// For some reason, eslint thinks ScrollBehavior is undefined...
	// eslint-disable-next-line no-undef
	function scrollChatBottom(behavior?: ScrollBehavior): void {
		elemChat.scrollTo({ top: elemChat.scrollHeight, behavior });
	}

	function onPromptKeydown(event: KeyboardEvent): void {
		if (["Enter"].includes(event.code)) {
			event.preventDefault();
			sendMessage();
		}
	}

	function setMaxChatHeight() {
		elemChat.style.maxHeight = "none";
		// get all the children of elmChat
		// @ts-ignore
		const display = elemChatContent.style.display;
		elemChatContent.style.display = "none";
		elemChat.style.maxHeight = `${elemChat.offsetHeight}px`;
		elemChatContent.style.display = display;
		scrollChatBottom();
	}

	onMount(() => {
		setMaxChatHeight();
	});

	function getTimestamp(timestamp?: string): string {
		let date;

		if (timestamp) {
			date = new Date(timestamp);
			date = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
		} else {
			date = new Date();
		}

		return date.toLocaleString("en-US", {
			hour: "numeric",
			minute: "numeric",
			hour12: true,
		});
	}

	function addMessage(newMessage: Message): void {
		// Update the message feed
		messageFeed = [...messageFeed, newMessage];
		// Smooth scroll to bottom
		// Timeout prevents race condition
		setTimeout(() => {
			scrollChatBottom("smooth");
		}, 0);
	}

	function sendMessage(): void {
		const newMessage: Message = {
			text: currentMessage,
			receiverId: currentUser.id,
			senderId: get(authStore)!.id,
		};
		addMessage({
			...newMessage,
			createdAt: getTimestamp(),
		});
		connection.invoke("SendMessage", newMessage);
		// Clear prompt
		currentMessage = "";
	}

	export let data: PageData;
	$: ({ connection } = data);

	$: if (connection) {
		// clear any previous event handlers
		connection.off("ReceiveMessage");

		// on receive message
		connection.on("ReceiveMessage", (message: Message) => {
			message.createdAt = getTimestamp();
			addMessage(message);
		});
	}
</script>

<svelte:window on:resize={setMaxChatHeight} />

<!-- Chat -->
<div class="flex flex-col justify-between">
	<!-- Conversation -->
	<section bind:this={elemChat} class="flex-initial h-full overflow-y-scroll">
		<div class="p-4 space-y-4" bind:this={elemChatContent}>
			{#each messageFeed as bubble}
				<div
					class="gap-2 flex flex-row"
					class:justify-end={bubble.senderId === currentUser.id}
				>
					<div
						class="card p-4 space-y-2 w-fit"
						class:variant-soft-primary={bubble.senderId ===
							currentUser.id}
						class:rounded-tr-none={bubble.senderId ===
							currentUser.id}
						class:rounded-tl-none={bubble.senderId !==
							currentUser.id}
					>
						<header class="flex justify-between items-center">
							<p class="font-bold me-2">
								{bubble.senderId === currentUser.id
									? currentUser.email
									: "You"}
							</p>
							<small class="opacity-50">
								{bubble.createdAt}
							</small>
						</header>
						<p>{bubble.text}</p>
					</div>
				</div>
			{/each}
		</div>
	</section>
	<!-- Prompt -->
	<section class="border-t border-surface-500/30 p-4 h-fit bottom-0">
		<div
			class="input-group input-group-divider grid-cols-[auto_1fr_auto] rounded-container-token"
		>
			<button class="input-group-shim">+</button>
			<textarea
				bind:value={currentMessage}
				class="bg-transparent border-0 ring-0"
				name="prompt"
				id="prompt"
				placeholder="Write a message..."
				rows="1"
				on:keydown={onPromptKeydown}
			/>
			<button
				class={currentMessage
					? "variant-filled-primary"
					: "input-group-shim"}
				on:click={sendMessage}
			>
				<i class="fa-solid fa-paper-plane" />
			</button>
		</div>
	</section>
</div>

<style lang="postcss">
</style>
