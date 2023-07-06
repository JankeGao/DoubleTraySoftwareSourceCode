<template>
  <canvas ref="elCanvas" :width="detail.style.position.w" :height="detail.style.position.h">
    Your browser does not support the HTML5 canvas tag.
  </canvas>
</template>

<script>
import canvasView from './ViewCanvas'

export default {
  name: 'ViewTriangle',
  extends: canvasView,
  mounted() {
    this.onResize()
  },
  methods: {
    drapTriangle(x1, y1, x2, y2, x3, y3, color, type) {
      var el = this.$refs.elCanvas
      var ctx = el.getContext('2d')
      ctx.beginPath()
      ctx.moveTo(x1, y1)
      ctx.lineTo(x2, y2)
      ctx.lineTo(x3, y3)
      ctx[type + 'Style'] = color
      ctx.closePath()
      ctx[type]()
    },
    onResize() {
      var w = this.detail.style.position.w
      var h = this.detail.style.position.h
      var el = this.$refs.elCanvas
      var ctx = el.getContext('2d')
      console.log('ctx')
      console.log(ctx)
      ctx.clearRect(0, 0, w, h)
      var x1 = w / 2
      var y1 = 0
      var x2 = 0
      var y2 = h
      var x3 = w
      var y3 = h
      var color = this.getForeColor()
      this.drapTriangle(x1, y1, x2, y2, x3, y3, color, 'fill')
    }
  }
}
</script>
