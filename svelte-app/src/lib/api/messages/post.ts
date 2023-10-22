import apiFetch from "$lib/api/apiFetch";

export default async function newMessage(message: Message): Promise<any> {
    const res = await apiFetch("/messages", {
        method: "POST",
        body: JSON.stringify(message)
    });
    return await res.json();
}