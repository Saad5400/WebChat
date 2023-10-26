import getUsers from "$lib/api/users/get";
import authStore from "$lib/stores/authStore.store.";
import { redirect, type Load } from "@sveltejs/kit";
import { get } from "svelte/store";
import * as signalR from "@microsoft/signalr";
import { PUBLIC_API_URL } from "$env/static/public";
import redirectStore from "$lib/stores/redirectStore.store";

export const load: Load = async ({ url }) => {
    if (!get(authStore)) {
        redirectStore.set(url.pathname);
        throw redirect(302, "/");
    }

    try {
        const userRes = await getUsers(get(authStore)?.email);
        if (userRes.ok === false) {
            authStore.set(null);
            throw redirect(302, "/");
        }

        const userData: User[] = await userRes.json();
        if (userData.length === 0) {
            authStore.set(null);
            throw redirect(302, "/");
        }
    } catch (e) {
        authStore.set(null);
        throw redirect(302, "/");
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(PUBLIC_API_URL + "/hubs/chat", {
            accessTokenFactory: () => get(authStore)!.accessToken!,
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Authorization": `Bearer ${get(authStore)!.accessToken}`,
            },
            withCredentials: false,
        })
        .withAutomaticReconnect()
        .build();

    await connection.start();

    return {
        connection,
    };
};