import apiFetch from "$lib/api/apiFetch";

export default async function users(email: string, id: string) {
    const res = await apiFetch(`/users?email=${email}&id=${id}`, {
        method: "GET"
    });
    return await res.json();
}