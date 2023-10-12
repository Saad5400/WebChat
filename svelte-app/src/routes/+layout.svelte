<script lang="ts">
	import { ProgressBar, autoModeWatcher } from "@skeletonlabs/skeleton";
	import { navigating } from "$app/stores";
	// import authStore from "$lib/stores/authStore.store.";
	import "../app.postcss";

	// authStore.subscribe((token) => {
	// 	console.log("authStore", token);
	// });
</script>

<svelte:head
	>{@html `<script>${autoModeWatcher.toString()} autoModeWatcher();</script>`}</svelte:head
>

{#if $navigating}
	<ProgressBar class="fixed top-0 w-full" meter="bg-surface-900/50" />
{/if}
<slot />
