<script lang="ts">
    import { onMount } from "svelte";
    import { page } from "$app/stores";
    import getUsers from "$lib/api/users/users";
    import { goto } from "$app/navigation";
    import postNewMessage from "$lib/api/messages/new";
    

    let elemChat: HTMLElement;
    let elemChatContent: HTMLElement;

    let currentMessage = "";
    let currentUser: User;
    let messageFeed: Message[] = [];

    page.subscribe((value) => {
        currentMessage = "";
        getUsers(value.params.email, "").then((users) => {
            currentUser = users[0];
        }).catch((error) => {
            goto("/chats");
        });
    });

    // For some reason, eslint thinks ScrollBehavior is undefined...
    // eslint-disable-next-line no-undef
    function scrollChatBottom(behavior?: ScrollBehavior): void {
        elemChat.scrollTo({ top: elemChat.scrollHeight, behavior });
    }

    function getCurrentTimestamp(): string {
        return new Date().toLocaleString("en-US", {
            hour: "numeric",
            minute: "numeric",
            hour12: true,
        });
    }

    function addMessage(): void {
        const newMessage: Message = {
            id: "",
            host: true,
            timestamp: `Today @ ${getCurrentTimestamp()}`,
            text: currentMessage,
            receiverId: currentUser.id,
        };
        postNewMessage(newMessage);
        // Update the message feed
        messageFeed = [...messageFeed, newMessage];
        // Clear prompt
        currentMessage = "";
        // Smooth scroll to bottom
        // Timeout prevents race condition
        setTimeout(() => {
            scrollChatBottom("smooth");
        }, 0);
    }

    function onPromptKeydown(event: KeyboardEvent): void {
        if (["Enter"].includes(event.code)) {
            event.preventDefault();
            addMessage();
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
</script>

<svelte:window on:resize={setMaxChatHeight} />

<!-- Chat -->
<div class="flex flex-col justify-between">
    <!-- Conversation -->
    <section bind:this={elemChat} class="flex-initial h-full overflow-y-scroll">
        <div class="p-4 space-y-4" bind:this={elemChatContent}>
            {#each messageFeed as bubble}
                {#if bubble.host === true}
                    <div class="gap-2 flex flex-row justify-start">
                        <div
                            class="card p-4 variant-soft rounded-tl-none w-fit space-y-2 min-w-[12rem]"
                        >
                            <header class="flex justify-between items-center">
                                <p class="font-bold">You</p>
                                <small class="opacity-50"
                                    >{bubble.timestamp}</small
                                >
                            </header>
                            <p>{bubble.text}</p>
                        </div>
                    </div>
                {:else}
                    <div class="gap-2 flex flex-row justify-end">
                        <div
                            class="card p-4 rounded-tr-none space-y-2 w-fit min-w-[12rem] variant-soft-primary"
                        >
                            <header class="flex justify-between items-center">
                                <p class="font-bold">{currentUser.email}</p>
                                <small class="opacity-50"
                                    >{bubble.timestamp}</small
                                >
                            </header>
                            <p>{bubble.text}</p>
                        </div>
                    </div>
                {/if}
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
                on:click={addMessage}
            >
                <i class="fa-solid fa-paper-plane" />
            </button>
        </div>
    </section>
</div>

<style lang="postcss">
</style>
