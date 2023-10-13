<script>
    import fetchChats from "$lib/api/chats/chats";
    import authStore from "$lib/stores/authStore.store.";
    import { get } from "svelte/store";

    if (!get(authStore).accessToken) {
        window.location.href = "/";
    }

    const chats = fetchChats();

    console.log(chats);
</script>

