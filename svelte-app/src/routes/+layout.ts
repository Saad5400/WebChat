import authStore from "$lib/stores/authStore.store.";
import redirectStore from "$lib/stores/redirectStore.store";
import { redirect, type Load } from "@sveltejs/kit";
import { get } from "svelte/store";

export const ssr = true;
export const prerender = true;

export const load: Load = async ({ url }) => {
    // If the user is LOGGED IN
    if (get(authStore)?.accessToken) {

        if (url.pathname == "/") {
            throw redirect(302, "/chats");
        }

    }
    // If the user is NOT logged in
    else {

        if (url.pathname.startsWith("/chats")) {
            redirectStore.set(url.pathname);
            throw redirect(302, "/");
        }

    }
};
