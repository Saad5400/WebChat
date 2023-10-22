import apiFetch from "$lib/api/apiFetch";

export default async function getMessages(id: string): Promise<any> {
    const res = await apiFetch(`/messages/${id}`, {
        method: "GET",
    });
    return await res.json();
}