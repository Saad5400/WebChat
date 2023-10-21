import apiFetch from "$lib/api/apiFetch";

export default async function searchUsers(query: string) {
    const res = await apiFetch("/users/search", {
        method: "POST",
        body: JSON.stringify(query),
    });
    return await res.json();
}