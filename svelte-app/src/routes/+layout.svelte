<script lang="ts">
	import { ProgressBar, autoModeWatcher } from "@skeletonlabs/skeleton";
	import { navigating, page } from "$app/stores";
	import loadingStore from "$lib/stores/loadingStore.store";
	import "../app.postcss";
	import { onDestroy, onMount } from "svelte";
	import authStore from "$lib/stores/authStore.store.";
	import redirectStore from "$lib/stores/redirectStore.store";
	import { get } from "svelte/store";
	import { goto } from "$app/navigation";

	onMount(() => {
		autoModeWatcher();
	});

	const unSubAuthStore = authStore.subscribe((store) => {
		if (store?.accessToken !== null && store?.email !== null) {
			if (get(redirectStore)) {
				goto(get(redirectStore));
			} else if ($page.url.pathname === "/") {
				goto("/chats");
			}
		} else {
			goto("/");
		}
	});
	onDestroy(() => {
		unSubAuthStore();
	});
</script>

<svelte:head>
	<title>Web Chat</title>
	{@html `<script>${autoModeWatcher.toString()} autoModeWatcher();</script>`}
</svelte:head>

{#if $navigating || $loadingStore}
	<ProgressBar class="fixed top-0 w-full" meter="bg-surface-900/50" />
{/if}
<slot />
