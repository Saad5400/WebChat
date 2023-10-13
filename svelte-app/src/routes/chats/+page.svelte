<script>
    import fetchChats from "$lib/api/chats/chats";
    import authStore from "$lib/stores/authStore.store.";
    import { get } from "svelte/store";

    if (!get(authStore).accessToken) {
        window.location.href = "/";
    }

    const chats = fetchChats();
</script>

<h1>Chats</h1>

{#await chats}
    <p>loading...</p>
{:then chats}
    {#each chats as chat}
        {chat.user.email}:
        {chat.lastMessage.text}
    {/each}
{:catch error}
    <p>error: {error.message}</p>
{/await}
