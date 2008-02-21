window.addEvent('domready', function(){
	var Tips1 = new Tips($$('.Tips1'), {
		className: 'custom',
		fixed: true,
		initialize:function(){
			this.fx = new Fx.Style(this.toolTip, 'opacity', {duration: 150, wait: false}).set(0);
		},
		onShow: function(toolTip) {
			this.fx.start(1);
		},
		onHide: function(toolTip) {
			this.fx.start(0);
		}
	});
}); 