<script lang="ts">
    import isEmail from "validator/es/lib/isEmail";
    import isStrongPassword from "validator/es/lib/isStrongPassword";

    const minInputLength = 1;

    let email = "";
    let password = "";

    $: validEmail = isEmail(email);
    $: validPassword = isStrongPassword(password, {
        minLowercase: 0,
        minUppercase: 0,
        minNumbers: 0,
        minSymbols: 0,
    });
</script>

<div
    class="flex items-center w-full min-h-[100dvh] max-w-xs sm:max-w-md md:max-w-xl lg:max-w-2xl xl:max-w-3xl mx-auto"
>
    <h2 class="text-center h2 gradient-heading1">
        Welcome to the
        <h1 class="h1 gradient-heading2">New WhatsApp</h1>
        <h3 class="h3">
            We provide complete privacy with a seamless experience
        </h3>
    </h2>
</div>
<div class="min-h-screen flex flex-row w-full items-center">
    <div
        class="w-full bg-surface-900 shadow-2xl p-6 mx-auto max-w-md sm:max-w-lg rounded-lg"
    >
        <h4 class="h4 mb-10 !font-normal gradient-heading1">
            So, what are you waiting for? <h2 class="h2 text-center">
                Register Now!
            </h2>
        </h4>
        <form class="flex flex-col gap-4">
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
                />
            </label>
        </form>
    </div>
</div>

<style lang="postcss">
    .gradient-text {
        @apply bg-clip-text text-transparent box-decoration-clone;
    }

    .gradient-heading1 {
        @apply gradient-text;
        /* Direction */
        @apply bg-gradient-to-br;
        /* Color Stops */
        @apply from-secondary-100 via-tertiary-500 to-secondary-100;
    }

    .gradient-heading2 {
        @apply gradient-text;
        /* Direction */
        @apply bg-gradient-to-tl;
        /* Color Stops */
        @apply from-primary-100 to-secondary-900;
    }
</style>
