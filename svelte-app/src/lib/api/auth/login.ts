import apiFetch from "$lib/api/apiFetch";

export default async function login(email: string, password: string) {
    const res = await apiFetch("/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });
    return await res.json();
}