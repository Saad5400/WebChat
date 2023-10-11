<script lang="ts">
	import { autoModeWatcher } from '@skeletonlabs/skeleton';
	import '../app.postcss';
</script>

<svelte:head>{@html `<script>${autoModeWatcher.toString()} autoModeWatcher();</script>`}</svelte:head>

<slot />
