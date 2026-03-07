/**
 * Line chart wrapper.
 *
 * @author BarrY
 * @version 1.0
 * @Update 20220118
 * @requires chartist-js
 */
;
(function($) {
  'use strict';

  $.TuCore.charts.JsDonutChart = {
    /**
     *
     *
     * @var Object _baseConfig
     */
    _baseConfig: {},

    /**
     *
     *
     * @var jQuery pageCollection
     */
    pageCollection: $(),

    /**
     * Initialization of Line chart wrapper.
     *
     * @param String selector (optional)
     * @param Object config (optional)
     *
     * @return jQuery pageCollection - collection of initialized items.
     */

    init: function(selector, config) {
      this.collection = selector && $(selector).length ? $(selector) : $();
      if (!$(selector).length) return;

      this.config = config && $.isPlainObject(config) ?
        $.extend({}, this._baseConfig, config) : this._baseConfig;

      this.config.itemSelector = selector;

      this.initCharts();

      return this.pageCollection;
    },

    initCharts: function() {

      var $self = this,
        collection = $self.pageCollection;


      this.collection.each(function(i, el) {

        var optFillColors = JSON.parse(el.getAttribute('data-fill-colors'));

        $(el).attr('id', 'donutCharts' + i);

        $('<style id="donutChartsStyle' + i + '"></style>').insertAfter($(el));

        var donutChartStyles = '',
          optSeries = JSON.parse(el.getAttribute('data-series')),
          optLabels = JSON.parse(el.getAttribute('data-labels')),
          optBorderWidth = $(el).data('border-width'),
          optStartAngle = $(el).data('start-angle'),
          optShowLabels = $(el).data('show-labels') ? $(el).data('show-labels') : false,
          optLabelCurrency = $(el).data('label-currency'),

          // Tooltips
          optIsShowTooltips = Boolean($(el).data('is-show-tooltips')),
          optTooltipBadgeMarkup = $(el).data('tooltip-badge-markup'),
          optIsTooltipsAppendToBody = Boolean($(el).data('is-tooltips-append-to-body')),
          optIsReverseData = Boolean($(el).data('is-tooltip-reverse-data')),
          optTooltipCustomClass = $(el).data('tooltip-custom-class'),
          optTooltipCurrency = $(el).data('tooltip-currency'),
          optSliceMargin = $(el).data('slice-margin') ? $(el).data('slice-margin') : 0,
          optCustomTooltips = {
            appendTooltipContent: function(tooltip, data, pluginOptions) {
              var title = document.createElement('div');
              title.setAttribute('class', 'chartist-tooltip-title');
              title.innerHTML = data.label;
              tooltip.appendChild(title);

              var seriesLabel;

              data.series.forEach(function(arr, i) {
                seriesLabel = pluginOptions.createSeriesLabel({
                  id: i,
                  idAlpha: String.fromCharCode(97 + i),
                  value: arr.value,
                  meta: arr.meta
                }, pluginOptions);

                tooltip.appendChild(seriesLabel);
              });
            },
            createSeriesLabel: function(options, pluginOptions, i) {
              var seriesLabel = document.createElement('div');

              seriesLabel.setAttribute('class', 'chartist-tooltip-series-labels');
              seriesLabel.style.color = optLineColors[options.id];
              seriesLabel.innerHTML = '<span class="chartist-tooltip-meta">' + options.meta + '</span><span class="chartist-tooltip-value">' + options.value + '</span>';

              return seriesLabel;
            }
          },
          tooltips = [],


          data = {
            series: optSeries ? optSeries : false,
            labels: optLabels ? optLabels : false
          },

          options = {
            donut: true,
            donutSolid: true,
            showLabel: optShowLabels,
            chartPadding: 0,
            labelOffset: 20,
            labelDirection: 'implode', // implode explode
            donutWidth: optBorderWidth + optSliceMargin,
            startAngle: optStartAngle + optSliceMargin,
            labelInterpolationFnc: function(value) {
              return value + ' ' + optLabelCurrency;
            },
            plugins: []
          };

        if (optIsShowTooltips) {
          Chartist.plugins = Chartist.plugins || {};

          Chartist.plugins.customTooltip = function(options) {
            var pluginOptions = Chartist.extend({}, optCustomTooltips, options);

            return function plugin(chart) {

              if (!(chart instanceof Chartist.Line)) {
                return;
              }

              var createTooltip = function(options) {
                var tooltip = document.createElement('div');

                tooltip.id = 'tooltip-' + i + '-' + options.id + '';
                tooltip.className = 'chartist-tooltip-custom ' + optTooltipCustomClass;

                tooltip.innerHTML = '<div class="chartist-tooltip-inner"></div>';

                return tooltip;
              };

              var createTooltips = function(data) {
                var tooltipsContainer = document.querySelector('body');

                var tooltipContainer,
                  tooltip,
                  seriesLabel;

                data.forEach(function(tooltipData, s) {
                  tooltipData.id = s;
                  tooltipContainer = createTooltip(tooltipData);

                  tooltip = tooltipContainer.querySelector('.chartist-tooltip-inner');
                  pluginOptions.appendTooltipContent(tooltip, tooltipData, pluginOptions);

                  tooltips[s] = tooltip;
                  tooltipsContainer.appendChild(tooltipContainer);
                });
              };

              chart.on('data', function(data) {
                var tooltipData = [];
                console.log(tooltips)
                data.data.series.forEach(function(series, s) {
                  series.forEach(function(value, i) {
                    tooltipData[i] = tooltipData[i] || {};
                    tooltipData[i].label = data.data.labels[i];
                    tooltipData[i].series = tooltipData[i].series || [];
                    tooltipData[i].series[s] = value;
                  })
                });

                createTooltips(tooltipData);
              });
            };
          };
  console.log(data)

          options.plugins[0] = Chartist.plugins.tooltip({
            currency: !Array.isArray(optTooltipCurrency) ? optTooltipCurrency : ' ',
            tooltipFnc: function(meta, value) {
              console.log(tooltipFnc)
              console.log(optTooltipCurrency)
              if (meta !== "undefined" && value !== "undefined") {
                if (Array.isArray(optTooltipCurrency)) {
                  if (optIsReverseData) {
                    if (optTooltipBadgeMarkup) {
                      return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + value + '</span>' +
                        '<span class="chartist-tooltip-meta">' + meta + '</span>';
                    } else {
                      return '<span class="chartist-tooltip-value">' + value + '</span>' +
                        '<span class="chartist-tooltip-meta">' + meta + '</span>';
                    }
                  } else {
                    if (optTooltipBadgeMarkup) {
                      return optTooltipBadgeMarkup + '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                        '<span class="chartist-tooltip-value">' + value + '</span>';
                    } else {
                      return '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                        '<span class="chartist-tooltip-value">' + value + '</span>';
                    }
                  }
                } else {
                  if (optIsReverseData) {
                    if (this.currency) {
                      if (optTooltipBadgeMarkup) {
                        if (optIsTooltipCurrencyReverse) {
                          return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + (value + this.currency) + '</span>' +
                            '<span class="chartist-tooltip-meta">' + meta + '</span>';
                        } else {
                          return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + (this.currency + value) + '</span>' +
                            '<span class="chartist-tooltip-meta">' + meta + '</span>';
                        }
                      } else {
                        if (optIsTooltipCurrencyReverse) {
                          return '<span class="chartist-tooltip-value">' + (value + this.currency) + '</span>' +
                            '<span class="chartist-tooltip-meta">' + meta + '</span>';
                        } else {
                          return '<span class="chartist-tooltip-value">' + (this.currency + value) + '</span>' +
                            '<span class="chartist-tooltip-meta">' + meta + '</span>';
                        }
                      }
                    } else {
                      if (optTooltipBadgeMarkup) {
                        return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + value + '</span>' +
                          '<span class="chartist-tooltip-meta">' + meta + '</span>';
                      } else {
                        return '<span class="chartist-tooltip-value">' + value + '</span>' +
                          '<span class="chartist-tooltip-meta">' + meta + '</span>';
                      }
                    }
                  } else {
                    if (this.currency) {
                      if (optTooltipBadgeMarkup) {
                        if (optIsTooltipCurrencyReverse) {
                          return optTooltipBadgeMarkup + '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                            '<span class="chartist-tooltip-value">' + (value + this.currency) + '</span>';
                        } else {
                          return optTooltipBadgeMarkup + '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                            '<span class="chartist-tooltip-value">' + (this.currency + value) + '</span>';
                        }
                      } else {
                        if (optIsTooltipCurrencyReverse) {
                          return '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                            '<span class="chartist-tooltip-value">' + (value + this.currency) + '</span>';
                        } else {
                          return '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                            '<span class="chartist-tooltip-value">' + (this.currency + value) + '</span>';
                        }
                      }
                    } else {
                      if (optTooltipBadgeMarkup) {
                        return optTooltipBadgeMarkup + '<span class="chartist-tooltip-meta">' + meta + '</span>' +
                          '<span class="chartist-tooltip-value">' + value + '</span>';
                      } else {
                        var newTemplate = !optTooltipBadgeMarkup ? '<span class="chartist-tooltip-value">' + value + '</span>' : '<span class="chartist-tooltip-meta">' + meta + '</span><span class="chartist-tooltip-value">' + value + '</span>';

                        return newTemplate;
                      }
                    }
                  }
                }
              } else if (optSeries) {
                if (this.currency) {
                  if (optTooltipBadgeMarkup) {
                    if (optIsTooltipCurrencyReverse) {
                      return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + (value + this.currency) + '</span>';
                    } else {
                      return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + (this.currency + value) + '</span>';
                    }
                  } else {
                    return '<span class="chartist-tooltip-value">' + (this.currency + value) + '</span>';
                  }
                } else {
                  if (optTooltipBadgeMarkup) {
                    return optTooltipBadgeMarkup + '<span class="chartist-tooltip-value">' + value + '</span>';
                  } else {
                    return '<span class="chartist-tooltip-value">' + value + '</span>';
                  }
                }
              } else {
                return false;
              }
            },
            class: optTooltipCustomClass + ' chartist-tooltip-' + i,
            appendToBody: optIsTooltipsAppendToBody
          })

        }
        var chart = new Chartist.Pie(el, data, options),
          isOnceCreatedTrue = 1;

        chart.on('created', function() {

          if (isOnceCreatedTrue == 1) {

            $(el).find('.ct-series').each(function(i2) {
              donutChartStyles += '#donutCharts' + i + ' .ct-series:nth-child(' + (i2 + 1) + ') .ct-slice-donut-solid {fill: ' + optFillColors[i2] + '}';
            });

            donutChartStyles += '#donutCharts' + i + ' .ct-series .ct-slice-donut-solid {stroke: #ffffff; stroke-width: ' + optSliceMargin + '}';


            $('#donutChartsStyle' + i).text(donutChartStyles);

          }

          isOnceCreatedTrue++;
        });

        chart.update();

        //Actions
        collection = collection.add($(el));
      });
    }
  };
})(jQuery);