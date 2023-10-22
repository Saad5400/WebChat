<script lang="ts">
    import { Avatar } from "@skeletonlabs/skeleton";
    import { onMount } from "svelte";

    let elemChat: HTMLElement;
    let elemChatContent: HTMLElement;
    let currentMessage = "";

    const lorem =
        "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quisquam, voluptatum.";

    // Messages
    let messageFeed: MessageFeed[] = [
        {
            id: 0,
            host: true,
            avatar: 48,
            name: "Jane",
            timestamp: "Yesterday @ 2:30pm",
            message: lorem,
            color: "variant-soft-primary",
        },
        {
            id: 1,
            host: false,
            avatar: 14,
            name: "Michael",
            timestamp: "Yesterday @ 2:45pm",
            message: lorem,
            color: "variant-soft-primary",
        },
        {
            id: 2,
            host: true,
            avatar: 48,
            name: "Jane",
            timestamp: "Yesterday @ 2:50pm",
            message: lorem,
            color: "variant-soft-primary",
        },
        {
            id: 3,
            host: false,
            avatar: 14,
            name: "Michael",
            timestamp: "Yesterday @ 2:52pm",
            message: lorem,
            color: "variant-soft-primary",
        },
    ];

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
        const newMessage = {
            id: messageFeed.length,
            host: true,
            avatar: 48,
            name: "Jane",
            timestamp: `Today @ ${getCurrentTimestamp()}`,
            message: currentMessage,
            color: "variant-soft-primary",
        };
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
                    <div class="grid grid-cols-[auto_1fr] gap-2">
                        <Avatar
                            src="https://i.pravatar.cc/?img={bubble.avatar}"
                            width="w-12"
                        />
                        <div
                            class="card p-4 variant-soft rounded-tl-none space-y-2"
                        >
                            <header class="flex justify-between items-center">
                                <p class="font-bold">{bubble.name}</p>
                                <small class="opacity-50"
                                    >{bubble.timestamp}</small
                                >
                            </header>
                            <p>{bubble.message}</p>
                        </div>
                    </div>
                {:else}
                    <div class="grid grid-cols-[1fr_auto] gap-2">
                        <div
                            class="card p-4 rounded-tr-none space-y-2 {bubble.color}"
                        >
                            <header class="flex justify-between items-center">
                                <p class="font-bold">{bubble.name}</p>
                                <small class="opacity-50"
                                    >{bubble.timestamp}</small
                                >
                            </header>
                            <p>{bubble.message}</p>
                        </div>
                        <Avatar
                            src="https://i.pravatar.cc/?img={bubble.avatar}"
                            width="w-12"
                        />
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
