import apiFetch from "$lib/api/apiFetch";

export default async function login(email: string, password: string): Promise<Response> {
    const res: Response = await apiFetch("/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });
    return res;
}