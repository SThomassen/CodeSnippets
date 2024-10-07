<template>
	<div class="webgl-content">
		<!-- <div id="unity-container"></div> -->
		<div id="unity-container" class="unity-desktop">
			<canvas id="unity-canvas" width=1280 height=720></canvas>
		</div>
		<div v-if="loaded == false">
			
			<div class='disclaimer'>
				<span>{{$t('disclaimers.loading_app') }}</span>
				<v-progress-linear :value="progress"></v-progress-linear>
			</div>
			<!-- <div class="unity-loader">
				<div class="bar">
				<div class="fill" v-bind:style="{ width: progress * 100 + '%'}"></div>
				</div>
			</div> -->
		</div>
		<!-- <div class="footer" v-if="hideFooter !== true">
			<a class="fullscreen" @click.prevent="fullscreen">Fullscreen</a>
		</div> -->
	</div>
</template>

<script>
import data from '@/config/unity'
import Vue from 'vue'

export default {
    props: ['src', 'module', 'width', 'height', 'externalProgress', 'unityLoader', 'hideFooter'],
    name: 'UnityWebGL',
    data () {
      	return {
			iterate: 1,
			gameInstance: null,
			loaded: false,
			progress: 0,
			error: null,
			previousHeight: 0,
			messages: []
		}
	},
	created() {
		if (!this.eventBus) {
			this.eventBus = new Vue({
				data: {
					ready: false,
					load: false,
					finish: false
				}
			})
		}

		this.eventBus.$on('onProgress', percentage => {
			this.progress = percentage;
		});

		if (this.loaderUrl && !this.eventBus.load) {
			const script = document.createElement('SCRIPT')
			script.setAttribute('src', this.loaderUrl)
			script.setAttribute('async', '')
			script.setAttribute('defer', '')
			// var canvas = document.querySelector("#unity-canvas");
			
			this.eventBus.load = true
			script.onload = () => {
				let canvas = this.$el.querySelector('canvas');
				canvas.setAttribute("tabindex", "1");
        		canvas.focus(); 

				createUnityInstance(canvas, this.config, (progress) => {
					this.eventBus.$emit('onProgress', progress * 100);
					// progressBarFull.style.width = 100 * progress + "%";
				}).then((unityInstance) => {
					// console.log('finished', unityInstance);
					// loadingBar.style.display = "none";
					// fullscreenButton.onclick = () => {
					// 	unityInstance.SetFullscreen(1);
					// };
					this.gameInstance = unityInstance;
					this.loaded = true;
					this.eventBus.ready = true
					this.eventBus.$emit('onload')
				}).catch((message) => {
					alert(message);
				});
				
			}
			document.body.appendChild(script);
		} else {
			this.eventBus.ready = true
			this.eventBus.load = true
		}

		if (this.eventBus.ready) {
			this.instantiate()
		} else {
			this.eventBus.$on('onload', () => {
				this.instantiate()
			})
		}

		this.$bus.$on('onLoadCompleted', () => {
			setTimeout( () => {
				this.eventBus.finish = true;
				this.messages.push( null );
			}, 3000);
		})
    },
	watch: {
		messages (nv) {
			if (this.eventBus.ready && nv.length > 0){
				let message = nv.pop();
				if (message !== null && this.gameInstance !== null) {
					this.gameInstance.SendMessage(message.gameObject, message.method, message.param);
				}
			}
		}
	},
	computed: {
		loaderUrl() {
			return data.buildUrl(data.product.loaderUrl);
		},
		config() {
			return {
				dataUrl: data.buildUrl(data.product.dataUrl),
				frameworkUrl: data.buildUrl(data.product.frameworkUrl),
				codeUrl: data.buildUrl(data.product.codeUrl),
				streamingAssetsUrl: data.product.streamingAssetsUrl,
				companyName: data.product.companyName,
				productName: data.product.productName,
				productVersion: data.product.productVersion,
				showBanner: this.unityShowBanner,
			}
		},
	},
    methods: {
		setHeight(height) {
			let canvas = this.$el.querySelector('canvas');
			if (canvas === null) return;
			canvas.setAttribute('height',height);
		},
		fullscreen () {
			this.gameInstance.SetFullscreen(1)
		},
		message(gameObject, method, param) {
			if (param === null) {
				param = ''
			}

			console.log("!!send message");
			this.messages.push( { gameObject: gameObject, method: method, param: param } );
		},
		quit() {
			if (this.gameInstance === null || this.gameInstance === undefined) {
				this.gameInstance = null; 
				return;
			}
			this.gameInstance.Quit( () => {
				this.gameInstance = null; 
			});
		},
		instantiate() {
			this.message("Game Manager", "SetCaptureInput", "false");

			// if (typeof UnityLoader === 'undefined') {
			// 	let error = 'The UnityLoader was not defined, please add the script tag ' +
			// 	'to the base html and embed the UnityLoader.js file Unity exported or use "unityLoader" attribute for path to UnityLoader.js.'
			// 	console.error(error)
			// 	this.error = error
			// 	return;
			// }
			// if (this.src === null) {
			// 	let error = 'Please provice a path to a valid JSON in the "src" attribute.'
			// 	console.error(error)
			// 	this.error = error
			// 	return
			// }
			// let params = {}
			// if (this.externalProgress) {
			// 	params.onProgress = UnityProgress
			// } else {
			// 	params.onProgress = ((gameInstance, progress) => {
			// 		this.loaded = (progress === 1)
			// 		this.progress = progress
			// 	})
			// }

			// params.Module = { onRuntimeInitialized: this.unityReady };
			// this.gameInstance = UnityLoader.instantiate('unity-container', this.src, params);
		},
		unityReady() {
			this.eventBus.$emit('onfinish');
		},
		unityShowBanner(msg, type) {
			// var div = document.createElement('div');
			// div.innerHTML = msg;
			// warningBanner.appendChild(div);
			// if (type == 'error') div.style = 'background: red; padding: 10px;';
			// else {
			// if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
			// 	setTimeout(function() {
			// 		warningBanner.removeChild(div);
			// 		updateBannerVisibility();
			// 	}, 5000);
			// }
        	this.updateBannerVisibility();
      	},
	  	updateBannerVisibility() {
          	// warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
		}
	}
}
</script>

<style>
.disclaimer {
	position: absolute;
	top: 128px;
	left: 50%;
	transform: translate(-50%, -50%);
	color: #212121;
}

#unity-container {
	position: relative!important;
	text-align: center!important;
	background:#dadada!important;
}
</style>