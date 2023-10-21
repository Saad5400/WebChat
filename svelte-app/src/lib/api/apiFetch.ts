import { PUBLIC_API_URL } from "$env/static/public";
import authStore from "$lib/stores/authStore.store.";
import { get } from "svelte/store";
import refresh from "./auth/refresh";

export default async function apiFetch(path: string, options: RequestInit = {}, tryRefresh = true): Promise<Response> {
    options = {
        ...options,
        headers: {
            ...options.headers,
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Access-Control-Allow-Origin": "*",
            "Authorization": `Bearer ${get(authStore)?.accessToken}`,
        },
    };
    try {
        const res = await fetch(PUBLIC_API_URL + path, options);
        return res;
    } catch (err) {
        if (!tryRefresh) throw err;
        
        await refresh();
        return apiFetch(path, options, false);
    }
}