# from http://programanddesign.com/js/jquery-select-text-range/
$.fn.highlightInputSelectionRange = (start, end) ->
    this.each () ->
        if this.setSelectionRange # non-IE
            this.focus()
            this.setSelectionRange start, end
        else if this.createTextRange # IE
            range = this.createTextRange()
            range.collapse true
            range.moveEnd 'character', end
            range.moveStart 'character', start
            range.select()


String.prototype.hashCode = () ->
    hash = 0 #, i, char;
    if (this.length == 0)
        hash
    else
        for element, i in this
            #   for (i = 0, l = this.length; i < l; i++) {
            char = this.charCodeAt(i)
            hash = ((hash << 5) - hash) + char
            hash |= 0 #; // Convert to 32bit integer
        hash


# extend lodash to easily provide globally-accessible utility functions

# Updates the array to match the contents of the second array
# using the matcher function to determine identity.
# the important thing is that the *contents* of the array is updated
# and that any existing matching objects are not replaced
_.mixin update: (array, newer, matcher) ->
    
    # pairs of (old, new) items
    #existing = (o, n for n in newer when
    #newer.forEach (item) ->
    #    if matcher item, 
        
        