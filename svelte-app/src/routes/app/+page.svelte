<script>
    import fetchMessages from "$lib/api/messages/messages";
    import authStore from "$lib/stores/authStore.store.";
    import { get } from "svelte/store";

    if (!get(authStore).accessToken) {
        window.location.href = "/";
    }

    const messages = fetchMessages();

    console.log(messages);
</script>

{#await messages}
    <p>Loading...</p>
{:then messages}
    {#each messages as chat}
        <h2>
            {chat[0].sender.email}
        </h2>
        <p>{chat[chat.length - 1].text}</p>
    {/each}
{:catch error}
    <p>{error.message}</p>
{/await}
