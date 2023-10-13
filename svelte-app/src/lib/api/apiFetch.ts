import { PUBLIC_API_URL } from "$env/static/public";
import authStore from "$lib/stores/authStore.store.";
import { get } from "svelte/store";
import refresh from "./auth/refresh";

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
    try {
        const res = await fetch(PUBLIC_API_URL + path, options);
        return res;
    } catch (err) {
        await refresh();
        return apiFetch(path, options);
    }
    // if (res.status === 401) {
    //     await refresh();
    //     return apiFetch(path, options);
    // }
    // return res;
}