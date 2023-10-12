import { PUBLIC_API_URL } from "$env/static/public";
import authStore from "$lib/stores/authStore.store.";
import { get } from "svelte/store";

export default async function apiFetch(path: string, options: RequestInit = {}) {
    options = {
        ...options,
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Access-Control-Allow-Origin": "*",
            "Authorization": `Bearer ${get(authStore)?.accessToken}`,
            ...options.headers,
        },
    };
    return fetch(PUBLIC_API_URL + path, options);
}