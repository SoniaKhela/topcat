(function() {
  $.fn.highlightInputSelectionRange = function(start, end) {
    return this.each(function() {
      var range;
      if (this.setSelectionRange) {
        this.focus();
        return this.setSelectionRange(start, end);
      } else if (this.createTextRange) {
        range = this.createTextRange();
        range.collapse(true);
        range.moveEnd('character', end);
        range.moveStart('character', start);
        return range.select();
      }
    });
  };

  String.prototype.hashCode = function() {
    var char, element, hash, i, _i, _len;
    hash = 0;
    if (this.length === 0) {
      return hash;
    } else {
      for (i = _i = 0, _len = this.length; _i < _len; i = ++_i) {
        element = this[i];
        char = this.charCodeAt(i);
        hash = ((hash << 5) - hash) + char;
        hash |= 0;
      }
      return hash;
    }
  };

  _.mixin({
    updateArrayWithNewContent: function(array, newer) {
      var copy;
      if (!array) {
        return;
      }
      console.log(array);
      copy = array.slice();
      array.length = 0;
      return _.forEach(newer, function(item) {
        var old;
        old = _.findWhere(copy, item);
        if (old) {
          return array.push(old);
        } else {
          return array.push(item);
        }
      });
    }
  });

}).call(this);

//# sourceMappingURL=utilities.js.map
