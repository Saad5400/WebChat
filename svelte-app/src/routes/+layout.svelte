<script lang="ts">
	import { ProgressBar, autoModeWatcher } from "@skeletonlabs/skeleton";
	import { navigating } from "$app/stores";
	import "../app.postcss";
</script>

<svelte:head
	>{@html `<script>${autoModeWatcher.toString()} autoModeWatcher();</script>`}</svelte:head
>

{#if $navigating}
	<ProgressBar class="fixed top-0 w-full" meter="bg-surface-900/50" />
{/if}
<slot />
