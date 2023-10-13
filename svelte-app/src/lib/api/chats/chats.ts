import apiFetch from "$lib/api/apiFetch";

export default async function chats() {
    const res = await apiFetch("/chats", {
        method: "GET",
    });
    return await res.json();
}