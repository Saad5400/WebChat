<script lang="ts">
    import register from "$lib/api/auth/register";
    import authStore from "$lib/stores/authStore.store.";
    import token from "$lib/stores/authStore.store.";
    import isEmail from "validator/es/lib/isEmail";
    import isStrongPassword from "validator/es/lib/isStrongPassword";
    // import authStore from "$lib/stores/auth.store.";

    const minInputLength = 1;

    let email = "";
    let password = "";
    let errorMessage = "";

    $: validEmail = isEmail(email);
    $: validPassword = isStrongPassword(password, {
        minLowercase: 0,
        minUppercase: 0,
        minNumbers: 0,
        minSymbols: 0,
    });

    // scrolling
    let y = 0;
    let lastY = 0;

    let registerElement: HTMLFormElement;
    let landingElement: HTMLDivElement;

    let scrolling = false;

    const scrollToRegister = (y: number) => {
        let dy = lastY - y;
        lastY = y;

        if (!scrolling && (dy < -10 || dy > 10)) {
            scrolling = true;
            setTimeout(() => {
                scrolling = false;
            }, 500);
            if (dy < -10)
                scrollTo(0, registerElement.getBoundingClientRect().top + y);
            if (dy > 10)
                scrollTo(0, landingElement.getBoundingClientRect().top + y);
        }
    };

    $: scrollToRegister(y);

    // register
    const registerSubmit = (e: Event) => {
        e.preventDefault();
        if (validEmail && validPassword) {
            register(email, password).then((res) => {
                console.log(res);
                if (res.status === 401) {
                    errorMessage = "Invalid credentials";
                } else {
                    authStore.set({
                        accessToken: res.accessToken,
                        refreshToken: res.refreshToken,
                    });
                }
            });
        }
    };

    authStore.subscribe(({ accessToken, refreshToken }) => {
        if (accessToken) {
            window.location.href = "/chats";
        }
    });
</script>

<svelte:window bind:scrollY={y} />

<div
    class="flex items-center w-full min-h-[100dvh] max-w-xs sm:max-w-md md:max-w-xl lg:max-w-2xl xl:max-w-3xl mx-auto"
    bind:this={landingElement}
>
    <h2 class="text-center h2 gradient-text gradient-heading1">
        Welcome to the
        <h1 class="h1 gradient-text gradient-heading2">New WhatsApp</h1>
        <h3 class="h3">
            We provide complete privacy with a seamless experience
        </h3>
    </h2>
</div>
<div class="min-h-screen flex flex-row w-full items-center">
    <div
        class="w-full bg-surface-900 shadow-2xl p-6 mx-auto max-w-md sm:max-w-lg rounded-lg"
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
                    class:input-error={email?.length >= minInputLength &&
                        !validEmail}
                    bind:value={email}
                    required
                    minlength={minInputLength}
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
                    class:input-error={password?.length >= minInputLength &&
                        !validPassword}
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
        @apply from-secondary-100 via-tertiary-500 to-secondary-100;
    }

    .gradient-heading2 {
        /* Direction */
        @apply bg-gradient-to-tl;
        /* Color Stops */
        @apply from-primary-100 to-secondary-900;
    }
</style>
