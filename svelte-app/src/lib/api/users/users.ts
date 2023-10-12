import apiFetch from "$lib/api/apiFetch";

export default async function users(userId: string = "") {
    const res = await apiFetch("/users", {
        method: "GET",
        body: userId ? JSON.stringify({ userId }) : undefined,
    });
    return await res.json();
}