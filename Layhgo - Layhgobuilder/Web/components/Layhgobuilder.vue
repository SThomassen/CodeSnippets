<template>
<span>
    <div v-if='initialized || (!initialized && mainPage)' v-show='mainPage' style='height:100%; overflow:hidden!important;'>
        <div v-if='!mobile' style='height:95%;' id="builder-canvas">
            <unity
                ref="unityInstance"
                v-if='activated'
                :hideFooter='true'>
            </unity>

            <!-- <l-banner></l-banner> -->
        </div>
        <div v-else style='height:95%'>
            <v-img height="100%" src='cover_layhgobuilder.png'>
                <v-row align='center' justify='center' style='height:100%'>
                    <v-col style="text-align:center;">
                        <span class='subtitle-1 font-weight-bold'>{{$t('disclaimer.support_mobile')}}</span>
                        <br>
                        <span class='subtitle-1 font-weight-bold'>{{$t('disclaimer.support_desktop')}}</span>
                    </v-col>
                </v-row>
            </v-img>
        </div>
    </div>
</span>
</template>

<script>
import Unity from '~/components/Unity'
import Banner from '@/components/ads/banner'

import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            interval: 600000, // every 10 min
            initialized: false,
            activated: true,
            target: "Game Manager"
        }
    },
    created() {
        this.$bus.$on('onToggleTheme', () => {
			this.onChangeTheme();
        });
        
        this.$bus.$on('onOpenProject', id => {
            this.openProject(id);
        });

        this.$bus.$on('onOpenLogin', () => {
			this.onOpenForm();
		})

		this.$bus.$on('onOpenSignup', () => {
			this.onOpenForm();
		})

		this.$bus.$on('onOpenForgot', () => {
			this.onOpenForm();
		})

		this.$bus.$on('onOpenFieldlabLicense', () => {
			this.onOpenForm();
		})

		this.$bus.$on('onOpenEnterpriseLicense', () => {
			this.onOpenForm();
		})

        this.$bus.$on('onCloseForm', () => {
			this.onCloseForm();
		})

        this.$bus.$on('onAuthChanged', id => {
            this.authenticateUser();
        })

        let self = this;
        window.addEventListener('message', e => {
            var message = e.data;

            switch(message) {
                case "requestToken":
                    this.authenticateUser();
                    break;
                case "openSignin":
                    self.$bus.$emit('onOpenLogin');
                    break;
                case "loadCompleted":
                    this.initialized = true;
                    self.setCanvasHeight();
                    self.onChangeTheme();
                    this.$bus.$emit('onLoadCompleted');
                    break;
                case "openTerms":
                    self.window.open('assets/Gebruikersvoorwaarden_Layhgo.pdf','_blank','fullscreen=yes');
                    break;
            }
        });
    },
    mounted() {
        window.addEventListener('resize', this.setCanvasHeight);
        setTimeout( () => { this.setCanvasHeight() }, 2000);
        setInterval( this.notifyInterval, this.interval);
    },
    watch: {
		'$route' (to,from) {
			if (to.path === '/') {
				this.$nextTick( () => {
                    this.setCanvasHeight();
                    setTimeout( () => { this.setCanvasHeight() }, 1000);
                    setInterval( this.notifyInterval, this.interval);
				});
            } else {
                clearInterval();
            }
		}
	},
	components: {
        'unity': Unity,
        'l-banner': Banner,
    },
    computed: {
		mainPage() {
            let homePage = this.localePath('/');
            let page = this.$route.path === homePage || this.$route.path === '/';

            if (this.$route.path === `${homePage}/`) {
                var base_url = window.location.origin;
                window.location.replace(base_url);
            }
			return page;
        },
        mobile() {
			 return (typeof window.orientation !== "undefined") || (navigator.userAgent.indexOf('IEMobile') !== -1);
		},
        user() {
			let user = this.$store.getters['auth/user'];
			return (user) ? user : {};
		},
    },
    methods: {
        setCanvasHeight() {
            if (!this.mainPage) return;

			let unityCanvas = this.$el.querySelector("canvas");
			let unityContainer = this.$el.querySelector("#unity-container");
			if (unityCanvas === null || unityContainer === null) return;

			unityCanvas.setAttribute("style", "background:#dadada!important; height:0px;");
			unityCanvas.style.height = "0px";

			unityContainer.setAttribute("style", "background:#dadada!important; height:0px;");
			unityContainer.style.height = "0px";

			let buildCanvas = this.$el.querySelector('#builder-canvas');
			if (buildCanvas === null) return;

			let canvasWidth = buildCanvas.getBoundingClientRect().width;
			let canvasHeight = buildCanvas.getBoundingClientRect().height;			
			
			let adCanvas = this.$el.querySelector('#adImage');
			let adHeight = 0;
			if (adCanvas !== null) {
				adHeight = adCanvas.getBoundingClientRect().height;
			}

            let total = canvasHeight - adHeight;

			unityCanvas.setAttribute("style", "background:#dadada!important; height:"+ total +"px; width:"+canvasWidth+"px;");
			unityCanvas.style.height = total;

			unityContainer.setAttribute("style", "background:#dadada!important; height:"+ total +"px; width:"+canvasWidth+"px;");
			unityContainer.style.height = total;

            let hooperList = this.$el.querySelector('.hooper-list');
			if (hooperList === null) return;
			hooperList.setAttribute("style", `height: ${adHeight}px`);
		},
		onChangeTheme() {
			let theme = "light";
            if (this.$vuetify.theme.dark) {
                theme = "dark";
			}
            
			if (this.activated && this.$refs.unityInstance && this.activated) {
				this.$refs.unityInstance.message(this.target, "SetTheme", theme);
			}
		},
        onOpenForm() {
            console.log('onOpenForm');
            if (this.$refs.unityInstance) {
                this.$refs.unityInstance.message(this.target, "SetCaptureInput", "false");
            }
        },
        onCloseForm() {
            console.log('onCloseForm');
            if (this.$refs.unityInstance) {
                this.$refs.unityInstance.message(this.target, "SetCaptureInput", "true");
            }
        },
        async authenticateUser() {
            console.log("auth changed");
            if (!this.activated || this.mobile) { return; }

            let user = firebase.auth().currentUser;
            console.log("auth user", user);

            let token = this.$store.getters['auth/token'];
            if (user !== undefined && token === undefined) {
                token = await firebase.auth().currentUser?.getIdToken();
                this.$store.dispatch('auth/setToken', user);
            }
            console.log("token", token);
            // if (token === null) { return; }
            
            if (this.$refs.unityInstance) {
                console.log("send token");
                this.$refs.unityInstance.message(this.target, "AuthenticateUser", token ? token : null);
            }
        },
		openProject(id) {
            let path = this.localePath({ path: '/', params: { }, query: { } });
			this.$router.push(path, 
            success => {
                this.$nextTick( () => {
				    this.$refs.unityInstance.message(this.target, "OpenProject", id);
                })
            }, 
            () => {

            });
        },
        notifyInterval() {
            this.$bus.$emit('onShowMessage', { icon: 'mdi-warning', message: this.$t('beta_message') });
        }
    }
}
</script>

<style>
    a:visited {
        color:inherit;
        text-decoration: none;
    }
    a:link {
        color:inherit;
        text-decoration: none;
    }

    canvas {
        height:100%;
    }

    #unity-container {
        height: 100%;
    }
</style>