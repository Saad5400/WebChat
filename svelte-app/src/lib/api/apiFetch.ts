import { PUBLIC_API_URL } from "$env/static/public";

export default async function apiFetch(path: string, options: RequestInit = {}) {
    options = {
        ...options,
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Access-Control-Allow-Origin": "*",
            ...options.headers,
        },
    };
    return fetch(PUBLIC_API_URL + path, options);
}