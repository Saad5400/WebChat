<script lang="ts">
</script>

<div class="grid w-full max-w-2xl min-h-[100dvh] mx-auto">
    <h2 class="self-center text-center h2 gradient-heading1">
        Welcome to the
        <h1 class="h1 gradient-heading2">New WhatsApp</h1>
        <h3 class="h3">
            We provide complete privacy with a seamless experience
        </h3>
    </h2>
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
