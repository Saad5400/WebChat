import apiFetch from "$lib/api/apiFetch";
import login from "./login";

export default async function register(email: string, password: string) {
    const res = await apiFetch("/auth/register", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });
    return login(email, password);
}