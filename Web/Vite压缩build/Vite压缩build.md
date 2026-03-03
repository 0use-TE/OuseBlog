### 前言

默认vite打包不会压缩，因此需要开启压缩功能，打包到br/gz

### 教程

#### 1. 安装插件

首先，你需要安装压缩插件。推荐使用 `vite-plugin-compression`：

```
npm install vite-plugin-compression -D
# 或者使用 yarn
yarn add vite-plugin-compression -D
```

#### 2. 在 `vite.config.ts` 中配置

你可以在配置中同时开启 Gzip 和 Brotli。Brotli (`.br`) 的压缩率通常比 Gzip 更高，但需要现代浏览器支持。

```javascript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import viteCompression from 'vite-plugin-compression'

export default defineConfig({
  plugins: [
    vue(),
    // 1. 生成 .gz 文件
    viteCompression({
      verbose: true,     // 是否在控制台输出压缩结果
      disable: false,    // 是否禁用
      threshold: 10240,  // 体积大于 10kb 才压缩
      algorithm: 'gzip', // 压缩算法
      ext: '.gz',        // 文件后缀
    }),
    // 2. 生成 .br 文件
    viteCompression({
      algorithm: 'brotliCompress',
      ext: '.br',
    })
  ]
})
```

### 关键参数说明：

* **`threshold`**: 建议设为 `10240` (10kb)。太小的文件压缩后体积反而可能变大，且解压耗时得不偿失。
* **`deleteOriginFile`**: 是否删除源文件。**通常建议设为 `false`**，因为如果浏览器不支持压缩格式，服务器还需要回退到普通文件。
