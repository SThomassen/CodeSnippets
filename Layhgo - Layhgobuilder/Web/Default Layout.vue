<template>
  	<v-app>
		<l-navbar></l-navbar>
		<l-sidebar></l-sidebar>

		<l-snackbar ref='snackbar'></l-snackbar>
		<l-changelog ref="changelog"></l-changelog>
		<l-auth ref='auth'></l-auth>

		<v-main>
			<nuxt />
			<l-layhgobuilder></l-layhgobuilder>
		</v-main>
		
		<l-footer></l-footer>
	</v-app>
</template>

<script>
import firebase from '@/plugins/firebase';

import Changelog from '@/shared/Dialog/Changelog'
import Snackbar from '@/shared/Notifications/Snackbar'

import Navbar from './navbar'
import Sidebar from './sidebar'
import Footer from './footer'

import Layhgobuilder from '@/components/Layhgobuilder'
import Auth from '~/components/auth/window'

export default {
	components: {
		'l-layhgobuilder': Layhgobuilder,
		'l-changelog': Changelog,
		'l-snackbar': Snackbar,
		'l-navbar': Navbar,
		'l-sidebar': Sidebar,
		'l-footer': Footer,
		'l-auth': Auth,
	},
	head () {
        return {
            title: "",
            meta: [
                // hid is used as unique identifier. Do not use `vmid` for it as it will not work
                { property:"og:title", content:"Layhgobuilder" },
                { property:"og:description", content:"Shape your own festival site or event within your web browser. Layhgobuilder is free for everyone, easy to use, and provides a quick overview of your design." },
                { property:"og:image", content:'https://scontent-amt2-1.xx.fbcdn.net/v/t1.0-9/75564636_100907858037152_1872682075509751808_n.jpg?_nc_cat=108&_nc_sid=174925&_nc_ohc=iduynvRA1UoAX9WKLAu&_nc_ht=scontent-amt2-1.xx&oh=5049bc47794168aef7c58b2f83025eee&oe=5EBE6A38' },
                { property:"og:url", content:"https://www.layhgobuilder.com" },

                { hid: 'description', name: 'description', content: 'Shape your own festival site or event within your web browser. Layhgobuilder is free for everyone, easy to use, and provides a quick overview of your design.' },
                { hid: 'og:image', name: 'og:image', content: 'https://scontent-amt2-1.xx.fbcdn.net/v/t1.0-9/75564636_100907858037152_1872682075509751808_n.jpg?_nc_cat=108&_nc_sid=174925&_nc_ohc=iduynvRA1UoAX9WKLAu&_nc_ht=scontent-amt2-1.xx&oh=5049bc47794168aef7c58b2f83025eee&oe=5EBE6A38' },

                { name: 'twitter:title', content: "Layhgobuilder" },
                { name: 'twitter:description', content: "Shape your own festival site or event within your web browser. Layhgobuilder is free for everyone, easy to use, and provides a quick overview of your design." },
                { name: 'twitter:image', content: 'https://scontent-amt2-1.xx.fbcdn.net/v/t1.0-9/75564636_100907858037152_1872682075509751808_n.jpg?_nc_cat=108&_nc_sid=174925&_nc_ohc=iduynvRA1UoAX9WKLAu&_nc_ht=scontent-amt2-1.xx&oh=5049bc47794168aef7c58b2f83025eee&oe=5EBE6A38' },
                { name: 'twitter:card', content: 'summary_large_image' }
            ]
        }
    },
	created() {
		this.$vuetify.theme.dark = (localStorage.theme === 'true');

		this.$store.dispatch('auth/getRoles');
		this.$store.dispatch('auth/getStats');

		// Register to listen to authentication changes
		firebase.auth().onAuthStateChanged( user => {
			if (user) {
				this.userLogin(user);
			}
			else {
				this.$bus.$emit('onOpenLogin');
			}
			
			this.$bus.$emit('onAuthChanged', user?.uid);
		});
	},
	mounted() {
		// Requests to open authentication windows
		this.$bus.$on('onOpenLogin', () => {
			this.$refs.auth.open(0);
		})

		this.$bus.$on('onOpenSignup', () => {
			this.$refs.auth.open(1);
		})

		this.$bus.$on('onOpenForgot', () => {
			this.$refs.auth.open(2);
		})

		this.$bus.$on('onShowMessage', message => {
			this.$refs.snackbar.show(message);
		})

		switch(this.$route.query.show)
		{
			case "login":
				this.$bus.$emit('onOpenLogin');
				break;
			case "signup":
				this.$bus.$emit('onOpenSignup');
				break;
			case "lost_password":
				this.$bus.$emit('onOpenForgot');
				break;
		}
	},
	computed: {
		user() {
			let user = this.$store.getters['auth/user'];
			return (user) ? user : {}
		}
	},
	methods: {
		openWindow(index) {
			if (this.$refs.auth === undefined)
				return;
		},
		async userLogin(user) {
			await this.$store.dispatch('auth/login', user);
			await this.$store.dispatch('auth/setToken', user);
			await this.$store.dispatch('auth/updateStatus', user.uid);
			await this.$store.dispatch('projects/getMyProjects', user.uid);

			if (user != null) {
				let message = this.$t('welcome', { name: user.displayName});
				this.$refs.snackbar.show({ message: message });
			}
		}
	}
}
</script>