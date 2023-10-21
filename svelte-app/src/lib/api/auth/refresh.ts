import { goto } from "$app/navigation";
import { PUBLIC_API_URL } from "$env/static/public";
import authStore from "$lib/stores/authStore.store.";
import { get } from "svelte/store";

export default async function refresh() {
    const res = await fetch(PUBLIC_API_URL + "/auth/refresh", {
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Access-Control-Allow-Origin": "*",
            "Authorization": `Bearer ${get(authStore)?.accessToken}`,
        },
        body: JSON.stringify({ refreshToken: get(authStore)?.refreshToken }),
        method: "POST",
    });
    if (res.status === 401) {
        authStore.set({ accessToken: "", refreshToken: "" });
        goto("/");
    }
    const data = await res.json();
    authStore.set({
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
    });
}