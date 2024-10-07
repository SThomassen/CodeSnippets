<template>
    <v-row no-gutters>
        <v-col cols='12'>
            <hooper 
                id="ad-canvas"
                :itemsToShow="1"
                :vertical="true" 
                :autoPlay="true"
                :playSpeed="timer"
                :mouseDrag='false'
                :touchDrag='false'
                :keysControl='false'
                :shortDrag='false'
                :hoverPause='true'
                :infiniteScroll="true"
                :wheelControl='false'
                style="height:100px;">
                <slide v-for='r in rows' :key='r' style="height:100px;">
                    <v-row no-gutters justify="center" id="adImage">
                        <v-col cols='2' v-for='i in length' :key='i'>

                            <a :href='ad(r,i).link' target='_blank'>
                                <v-img :src="ad(r,i).imageUrl" :aspect-ratio="4" max-height="100" />
                            </a>
                        </v-col>
                    </v-row>
                </slide>
            </hooper>
        </v-col>
    </v-row>
</template>

<script>
import { Hooper, Slide} from 'hooper';

export default {
    components: {
        Hooper,
        Slide
    },
    data() {
        return {
            length: 6,
            timer: 6667
        }
    },
    created() {
        this.getAds();
    },
    computed: {
        ads() {
            let ads = this.$store.getters['ads/web'];
            let today = new Date();
            ads = ads.filter( item => 
                ((item.start === undefined || new Date(item.start).getTime() < today.getTime() ) && 
                (item.end === undefined || new Date(item.end).getTime() > today.getTime()))
            );
            return (ads) ? ads : [];
        },
        rows() {
            if (this.ads.length === 0) return 0;
            let rows = this.ads.length / this.length;
            return Math.ceil(rows);
        }
    },
    methods: {
        getAds() {
            if (this.ads === undefined || this.ads.length <= 0) {
                this.$store.dispatch('ads/getAds');
            }
        },
        ad(row,index) {
            if (this.ads.length === 0) return "";

            let i = ((row - 1) * this.length) + (index - 1);
            i = (i >= this.ads.length) ? i - this.ads.length : i;
            let ad = this.ads[i];
            return (ad) ? ad : {};
        }
    }
}
</script>

<style>
.v-application ul {
    padding-left: 0px!important;
}

.hooper-slide {
    flex-shrink: 0;
    height: 100%;
    margin: 0;
    padding: 0;
}

.hooper {
    position: relative;
    width: 100%;
    max-height: 100px;
}

.hooper-list {
    overflow: hidden;
    width: 100%;
    height: 100%;
}

.hooper-track {
    display: flex;
    width: 100%;
    height: 100%;
    padding: 0;
    margin: 0;
}
.hooper.is-vertical .hooper-track {
    flex-direction: column;
    height:100px;
}

.hooper-sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
}
</style>