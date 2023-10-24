import { goto } from "$app/navigation";
import { PUBLIC_API_URL } from "$env/static/public";
import authStore from "$lib/stores/authStore.store.";
import { get } from "svelte/store";

export default async function refresh() {
    let res;
    try {
        res = await fetch(PUBLIC_API_URL + "/auth/refresh", {
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json",
                "Access-Control-Allow-Origin": "*",
                "Authorization": `Bearer ${get(authStore)?.accessToken}`,
            },
            body: JSON.stringify({ refreshToken: get(authStore)?.refreshToken }),
            method: "POST",
        });
        if (!res.ok) {
            authStore.set(null);
            goto("/");
        }
    }
    catch (e) {
        authStore.set(null);
        goto("/");
    }
    const authData = await res!.json();
    const storeData = get(authStore)!;
    authStore.set({
        accessToken: authData.accessToken,
        refreshToken: authData.refreshToken,
        email: storeData.email,
        id: storeData.id,
    });
}