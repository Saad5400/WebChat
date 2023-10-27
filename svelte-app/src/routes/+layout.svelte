<script lang="ts">
	import { ProgressBar, autoModeWatcher } from "@skeletonlabs/skeleton";
	import { navigating, page } from "$app/stores";
	import loadingStore from "$lib/stores/loadingStore.store";
	import "../app.postcss";
	import { onMount } from "svelte";

	onMount(() => {
		autoModeWatcher();
	});
</script>

<svelte:head>
	<title>Web Chat</title>
	<meta name="description" content="Chat with your friends privately, securely, and in real-time." />
</svelte:head>

{#if $navigating || $loadingStore}
	<ProgressBar class="fixed top-0 w-full" meter="bg-surface-900/50" />
{/if}
<slot />
