<template>
	<v-layout>
		<s-dialog ref="project"></s-dialog>

		<v-container>
			<v-row class="pb-6 mb-6">
				<v-col cols="10" offset="1">

					<v-row>
						<v-col cols="12" id="work">
							<v-card>
								<v-card-title>
									{{$t("work")}}
								</v-card-title>
								<s-work></s-work>
							</v-card>
						</v-col>

						<v-col cols="12" id="about">
							<v-card>
								<v-card-title>
									{{$t("about")}}
								</v-card-title>
								<v-row>
									<v-col lg="4" md="6" cols="12">
										<v-img class="ma-2" src="images/sjors-thomassen_orig.jpg"></v-img>
									</v-col>
									<v-col lg="4" md="6" cols="12">
										<s-info></s-info>
									</v-col>

									<v-col lg="4" cols="12">
										<s-skill></s-skill>
									</v-col>
								</v-row>
							</v-card>
						</v-col>

						<v-col cols="12" id="contact">
							<v-card id="scrolling-techniques-5">
								<v-row align="center" class="ml-2">
									<v-col cols="auto">
										<span class="title">{{$t("contact")}}</span>
									</v-col>
									<v-spacer/>
									<v-col cols="auto">
										<s-contact></s-contact>
									</v-col>
								</v-row>
							</v-card>
						</v-col>
					</v-row>
				</v-col>
			</v-row>
		</v-container>
	</v-layout>
</template>

<script>
	import Work from '@/components/work/work'
	import Info from "@/components/about/info"
    import Skill from "@/components/about/skill"
	import Contact from "@/components/contact/contact"
	import Dialog from "@/components/work/dialog"

	export default {
		mounted() {
			this.$bus.$on('openProject', (route) => {
				this.$refs.project.open(route);
			})

			let path = this.$route.name.substring(this.$route.name.indexOf("-") + 1);
			path = path.slice(0, path.indexOf("__"));

			if (path !== "index") {
				this.$refs.project.open(`/${path}`);
			}
		},
		name: 'IndexPage',
		components: {
			's-work': Work,
			's-info': Info,
			's-skill': Skill,
			's-contact': Contact,
			"s-dialog": Dialog
		}
	}
</script>
