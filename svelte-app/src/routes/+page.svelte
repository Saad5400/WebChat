<!-- YOU CAN DELETE EVERYTHING IN THIS PAGE -->

<script>
	export let data;
</script>

<div class="container min-h-screen max-w-2xl my-4 mx-auto">
	<div class="space-y-5">
		<h1 class="h1">Let's get cracking bones!</h1>
		<p>Start by exploring:</p>
		<p>
			{#await data.streamed.data}
				Loading ...
			{:then value}
				{@html value.content}
			{:catch error}
				{error.message}
			{/await}
		</p>
	</div>
</div>
