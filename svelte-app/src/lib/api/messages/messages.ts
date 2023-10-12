import apiFetch from "$lib/api/apiFetch";

export default async function messages(userId: string = "") {
    const res = await apiFetch("/messages", {
        method: "GET",
        body: userId ? JSON.stringify({ userId }) : undefined,
    });
    return await res.json();
}