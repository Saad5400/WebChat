import apiFetch from "$lib/api/apiFetch";

export default async function getUsers(email = "", id = ""): Promise<Response> {
    const res: Response = await apiFetch(`/users?email=${email}&id=${id}`, {
        method: "GET"
    });
    return res;
}