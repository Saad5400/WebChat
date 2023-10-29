<script lang="ts">
    import { goto } from "$app/navigation";
    import register from "$lib/api/auth/register";
    import getUsers from "$lib/api/users/get";
    import authStore from "$lib/stores/authStore.store.";
    import loadingStore from "$lib/stores/loadingStore.store";
    import redirectStore from "$lib/stores/redirectStore.store";
    import { get } from "svelte/store";

    let email = "";
    let password = "";
    let errorMessage = "";

    // scrolling
    let y = 0;
    let lastY = 0;

    let registerElement: HTMLFormElement;
    let landingElement: HTMLDivElement;

    let scrolling = false;
    const minDelta = 1;

    const scrollToRegister = (y: number) => {
        let dy = lastY - y;
        lastY = y;

        if (!scrolling && dy < -minDelta) {
            scrolling = true;

            setTimeout(() => {
                scrolling = false;
            }, 400);

            if (dy < -minDelta) {
                registerElement.focus();
                document.body.scrollTo(
                    0,
                    registerElement.getBoundingClientRect().top + y
                );
            }
        }
    };

    $: scrollToRegister(y);

    // register
    const registerSubmit = async (e: Event) => {
        e.preventDefault();
        loadingStore.set(true);
        const authRes = await register(email, password);

        if (authRes.ok === false) {
            errorMessage = "Invalid credentials";
            return;
        }

        const authData = await authRes.json();

        authStore.set({
            accessToken: authData.accessToken,
            refreshToken: authData.refreshToken,
        });

        const userRes = await getUsers(email);

        if (userRes.ok === false) {
            errorMessage = "Something went wrong";
            return;
        }

        const userData: User[] = await userRes.json();

        const user = userData[0];

        authStore.set({
            accessToken: authData.accessToken,
            refreshToken: authData.refreshToken,
            id: user.id,
            email: user.email,
        });

        const redirectUrl = get(redirectStore) || "/chats";
        redirectStore.set("");
        goto(redirectUrl);

        loadingStore.set(false);
    };
</script>

<svelte:body
    on:scroll={(e) => {
        // @ts-ignore
        y = e.target.scrollTop;
        if (scrolling) e.preventDefault();
    }}
/>

<div
    class="flex items-center w-full min-h-[100dvh] max-w-xs sm:max-w-md md:max-w-xl lg:max-w-2xl xl:max-w-3xl mx-auto"
    bind:this={landingElement}
>
    <div class="flex flex-col items-center gradient-text gradient-heading1">
        <h1 class="text-center h2">Welcome to the</h1>
        <h2 class="h1 gradient-text gradient-heading2">New WhatsApp</h2>
        <h3 class="h3 text-center">
            We provide complete privacy with a seamless experience
        </h3>
    </div>
</div>
<div class="min-h-screen flex flex-row w-full items-center">
    <div
        class="w-full bg-surface-50-900-token shadow-2xl p-6 mx-auto max-w-md sm:max-w-lg rounded-lg"
    >
        <h4 class="h4 mb-4 !font-normal gradient-text gradient-heading2">
            So, what are you waiting for? <h2
                class="h2 text-center leading-loose"
            >
                Register Now!
            </h2>
        </h4>
        <form
            method="post"
            class="flex flex-col gap-4"
            bind:this={registerElement}
            on:submit={registerSubmit}
        >
            <label class="label">
                <span>Email Address</span>
                <input
                    placeholder="my-email@example.com"
                    type="email"
                    name="email"
                    class="input variant-form-material"
                    bind:value={email}
                    required
                    on:focus={() => {
                        errorMessage = "";
                    }}
                />
            </label>
            <label class="label">
                <span>Password</span>
                <input
                    placeholder="********"
                    type="password"
                    name="password"
                    class="input variant-form-material"
                    min="8"
                    bind:value={password}
                    required
                    minlength="8"
                    on:focus={() => {
                        errorMessage = "";
                    }}
                />
            </label>
            <p class="text-xs text-error-500">{errorMessage}</p>
            <button
                type="submit"
                class="btn rounded-md bg-gradient-to-br variant-gradient-primary-tertiary text-primary-50"
            >
                Register/Login
            </button>
        </form>
    </div>
</div>

<style lang="postcss">
    .gradient-heading1 {
        /* Direction */
        @apply bg-gradient-to-br;
        /* Color Stops */
        @apply from-secondary-500 via-tertiary-500 to-secondary-900;
        @apply dark:from-secondary-100 dark:via-tertiary-500 dark:to-secondary-100;
    }

    .gradient-heading2 {
        /* Direction */
        @apply bg-gradient-to-tl;
        /* Color Stops */
        @apply from-tertiary-500 to-secondary-700;
        @apply dark:from-primary-100 dark:to-secondary-900;
    }

    html {
        overflow: hidden;
        width: 100%;
    }

    body {
        height: 100%;
        width: 100%;
        position: fixed;
        overflow-y: auto;
        -webkit-overflow-scrolling: touch;
        scroll-behavior: smooth;
    }
</style>
