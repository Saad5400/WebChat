import type { Load } from "@sveltejs/kit";

export const load: Load = async ({ fetch }) => {
    return {
        streamed: {
            data: new Promise(async (resolve) => {
                let data = await fetch("https://elcato.azurewebsites.net/api/pages/67");
                let json = await data.json();
                resolve(json);
            }),
        }
    }
};