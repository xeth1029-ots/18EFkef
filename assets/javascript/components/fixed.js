//指定エリア内に要素を追従させる関数（追従要素,追従エリア）
const areaFixedFunk = (fixedElm,fixedArea) => {
	//エリアチェック
	const areas = document.querySelectorAll(fixedArea);
	if(areas.length === 0) {
		return;
	}

	//追従チェック関数
	const checkFixed = (target,area) => {
		//要素の位置と高さを取得
		const startPosi = area.getBoundingClientRect().top;
		const targetHeight = target.clientHeight;
		const areaHeight = area.clientHeight;
		const endPosi = startPosi + areaHeight;

		//エリア内の処理
		if(0 > startPosi && targetHeight < endPosi) {
			target.classList.add('is-fixed');
			target.style.top = '';

		//エリアより上の処理
		} else if(0 <= startPosi) {
			target.classList.remove('is-fixed');
			target.style.top = '';

		//エリアより下の処理
		} else {
			target.classList.remove('is-fixed');
			//停止位置を設定
			target.style.top = (areaHeight - targetHeight) + 'px';
		}
	}

	//エリア毎に処理
	areas.forEach((area) => {
		//エリア内に追従要素が存在する場合のみ処理する
		const target = area.querySelector(fixedElm);
		if(target) {
			checkFixed(target,area);
			window.addEventListener('resize', ()=> {
				checkFixed(target,area);
			});
			window.addEventListener('scroll', ()=> {
				checkFixed(target,area);
			}, {passive: true});
		}
	});
}

//関数呼び出し（追従要素,追従エリア）
areaFixedFunk('.js-fixed-elm','.js-fixed-area');