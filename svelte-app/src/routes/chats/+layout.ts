import getUsers from "$lib/api/users/get";
import authStore from "$lib/stores/authStore.store.";
import { redirect, type Load } from "@sveltejs/kit";
import { get } from "svelte/store";

export const load: Load = async () => {
    if (!get(authStore)) {
        throw redirect(302, "/");
    }
    const userData: User[] = await (await getUsers(get(authStore)?.email)).json();
    if (userData.length === 0) {
        authStore.set(null);
        throw redirect(302, "/chats");
    }
};