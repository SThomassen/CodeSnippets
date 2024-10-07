<template>
	<v-app>
		<s-navbar></s-navbar>

		<v-main>
			<nuxt/>
		</v-main>

		<s-footer></s-footer>
	</v-app>
</template>

<script>
import NavBar from './navbar.vue'
import Footer from './footer.vue'

export default {
	name: 'DefaultLayout',
	data: () => ({
        
    }),
	created() {
		
	},
	components: {
		's-navbar': NavBar,
		's-footer': Footer
	},
	methods: {
		tmp() {
			this.drawer = !this.drawer;
		}
	}
}
</script>
