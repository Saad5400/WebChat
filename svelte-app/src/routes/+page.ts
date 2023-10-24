import authStore from "$lib/stores/authStore.store.";
import { redirect, type Load } from "@sveltejs/kit";
import { get } from "svelte/store";

export const load: Load = async () => {
    if (get(authStore) !== null) {
        throw redirect(302, "/chats");
    }
};