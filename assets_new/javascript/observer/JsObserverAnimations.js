// Main JsObserverAnimations Class
class JsObserverAnimations {
    constructor(options) {
        this.prefix = 'tu-an'
        this.documentHeight = document.documentElement.scrollHeight - document.documentElement.clientHeight
        this.defaultOptions = {
            root: null,
            element: `[data-${this.prefix}]`,
            enableCallback: false,
            callback: `data-${this.prefix}-call`,
            class: `${this.prefix}-inview`,
            initClass: `${this.prefix}-init`,
            threshold: 0.1,
            offset: {
                top: 0,
                right: 0,
                bottom: -40,
                left: 0,
            },
            direction: 'vertical',
            repeat: false,
            mirror: false,
            startEvent: 'DOMContentLoaded',
        }
        this.options = {...this.defaultOptions,
            ...options
        }
        this.observerOptions = {
            root: this.options.root,
            rootMargin: this.calculateOffset(this.options.offset),
            threshold: this.options.threshold,
        }
        this.animationElements = document.querySelectorAll(this.options.element) // Elements to animate
        this.callbackElements = document.querySelectorAll(`[${this.options.callback}]`) // Elements to call on intersect
        this.init()
    }


    init() {
        const completeEvent = new Event(`${this.prefix}:complete`)

        const animationObserverFunction = (entries) => {
            entries.forEach((entry) => {
                const dataAttr = entry.target.getAttribute('data-tu-an');

                if (!this.options.repeat && entry.isIntersecting) {
                    entry.target.classList.add(this.options.class)
                    observer.unobserve(entry.target)
                    return
                }

                entry.target.classList.toggle(this.options.class, entry.isIntersecting)
                if (entry.isIntersecting && dataAttr && dataAttr.includes('once')) entry.target.classList.add(`${this.prefix}-once`)
            })
        }


        const callbackObserverFunction = (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    const call = entry.target.getAttribute(this.options.callback)
                    window[call] && window[call]()
                    caller.unobserve(entry.target)
                }
            })
        }


        const startEventCallback = () => {
            this.animationElements.forEach((element) => {
                observer.observe(element)
                element.classList.add(this.options.initClass)
            })

            document.body.classList.add(`${this.prefix}-loaded`)
            window.dispatchEvent(completeEvent)
        }


        const startCallbackFunction = () => {
            this.callbackElements.forEach((element) => {
                caller.observe(element)
            })
        }


        const observer = new IntersectionObserver(animationObserverFunction, this.observerOptions)

        let caller = null
        this.options.enableCallback && (caller = new IntersectionObserver(callbackObserverFunction, this.observerOptions))

        window.addEventListener(this.options.startEvent, startEventCallback)
        this.options.enableCallback && window.addEventListener(this.options.startEvent, startCallbackFunction)
        window.dispatchEvent(completeEvent)
    }

    update() {
        this.init()
    }

    on(event, callback) {
        window.addEventListener(
            event,
            (e) => {
                callback(e)
            },
            false
        )
    }


    calculateOffset(data) {
        let obj = {...this.defaultOptions.offset,
            ...data
        }
        let offset = `${obj.top}px ${obj.right}px ${obj.bottom}px ${obj.left}px`
        let repeatOffset = `${this.documentHeight}px ${obj.right}px ${obj.bottom}px ${obj.left}px`

        if (this.options.repeat) {
            !this.options.mirror && (offset = repeatOffset)
        }

        return offset
    }
}