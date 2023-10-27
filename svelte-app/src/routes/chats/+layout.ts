import getUsers from "$lib/api/users/get";
import authStore from "$lib/stores/authStore.store.";
import type { Load } from "@sveltejs/kit";
import { get } from "svelte/store";
import * as signalR from "@microsoft/signalr";
import { PUBLIC_API_URL } from "$env/static/public";
import refresh from "$lib/api/auth/refresh";
import redirectStore from "$lib/stores/redirectStore.store";

export const load: Load = async ({ url }) => {

    await refresh();

    const accessToken = get(authStore)!.accessToken!;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(PUBLIC_API_URL + "/hubs/chat", {
            accessTokenFactory: () => accessToken,
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Authorization": `Bearer ${accessToken}`,
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