<template>
    <v-toolbar-items>
        <template v-for="item in menu" >
            <v-btn :x-small="breakpoint" plain :key="item.title" :to="{ path: localePath('/'), hash: item.route }">
                <span class="d-none d-sm-flex">{{$t(item.title)}}</span>
                <v-icon class="d-flex d-sm-none">{{item.icon}}</v-icon>
            </v-btn>
            <!-- <v-list-item :key="item.title" :to="localePath(item.route)">
                <template>
                    <v-list-item-icon>
                        <v-icon right>{{item.icon}}</v-icon>
                    </v-list-item-icon>
                    <v-list-item-title>
                        {{ $t(item.title) }}
                    </v-list-item-title>
                </template>
            </v-list-item> -->
        </template>
    </v-toolbar-items>
</template>

<script>
export default {
    computed: {
        menu() {
            return [
                {
                    title: "work",
                    auth: [],
                    icon: "mdi-gamepad-variant",
                    route: "work"
                },
                {
                    title: "about",
                    auth: [],
                    icon: 'mdi-rhombus',
                    route: "about"
                },
                {
                    title: "contact",
                    auth: [],
                    icon: "mdi-email",
                    route: "contact"
                }
            ]
        },
        breakpoint() {
            return this.$vuetify.breakpoint.xs;
        }
    },
    methods: {
        scrollTo(el) {
            const element = document.getElementById(el);
            element.scrollIntoView();
        }
    }
}
</script>