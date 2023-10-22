import apiFetch from "$lib/api/apiFetch";

export default async function chats(email = ""): Promise<any> {
    let res: Response;
    if (email)
        res = await apiFetch(`/chats/${email}`, {
            method: "GET",
        });
    else
        res = await apiFetch("/chats", {
            method: "GET",
        });
    return await res.json();
}